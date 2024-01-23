targetScope = 'resourceGroup'

param location string = resourceGroup().location
param _now string

// app service
resource nrWebAppPlan 'Microsoft.Web/serverfarms@2023-01-01' = {
	location: location
	name: 'nrWebAppPlan'
	kind: 'app'
	sku: {
		name: 'F1'
		tier: 'Free'
		size: 'F1'
		family: 'F'
		capacity: 0
	}
}
resource nrWebApp 'Microsoft.Web/sites@2021-01-15' = {
	name: 'NSMB-RNG'
	location: location
	kind: 'app'
	identity: {
		type: 'SystemAssigned'
	}
	properties: {
		serverFarmId: nrWebAppPlan.id
	}
}

// cosmos
resource nrCosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
	name: 'nsmb-rng-cosmos'
	kind: 'GlobalDocumentDB'
	location: location
	properties: {
		databaseAccountOfferType: 'Standard'
		enableFreeTier: true
		capacity: {
			totalThroughputLimit: 1000
		}
		locations: [
			{
				locationName: location
				failoverPriority: 0
				isZoneRedundant: false
			}
		]
	}

	// The "sqlDatabases" resource type name is misleading. This db will use NoSQL.
	resource db 'sqlDatabases' = {
		name: 'defaultDB'
		properties: {
			options: { throughput: 400 }
			resource: { id: 'defaultDB' } // required redundancy
		}

		resource container 'containers' = {
			name: 'defaultContainer'
			properties: {
				resource: {
					id: 'defaultContainer' // required redundancy
					partitionKey: {
						paths: [ '/mac' ]
					}
					indexingPolicy: {
						automatic: true
						excludedPaths: [{ path: '/*' }]
						includedPaths: [
							{ path: '/mac/?' }
							// _ts is indexed automatically. Manually specifying it does nothing.
							// (It wouldn't even show up in the indexing policy.)
						]
					}
				}
			}
		}
	}

	// Note: This is not a normal role assignment (not Microsoft.Authorization/roleAssignment)
	// and does nto assign a normal role. This is CosmosDB-specific RBAC. There are no normal
	// roles which grant access to the data inside containers.
	resource sqlRoleAssignment 'sqlRoleAssignments' = {
		name: guid('00000000-0000-0000-0000-000000000002', nrWebApp.id, db::container.id)
		properties:{
			principalId: nrWebApp.identity.principalId
			roleDefinitionId: '${nrCosmosAccount.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002'
			// Weird that it requires a special format, instead of just container.id.
			scope: '${nrCosmosAccount.id}/dbs/${db.name}/colls/${db::container.name}'
		}
	}
}

// Update the webapp app settings while keeping any default settings.
module appSettings 'appsettings.bicep' = {
	name: 'update-app-settings-${_now}'
	params: {
		webAppName: nrWebApp.name
		currentAppSettings: list(resourceId('Microsoft.Web/sites/config', nrWebApp.name, 'appsettings'), '2023-01-01').properties
		appSettings: {
			CosmosEndpointUri: nrCosmosAccount.properties.documentEndpoint
			CosmosDbName: 'defaultDB'
			CosmosContainerName: 'defaultContainer'
		}
	}
}
