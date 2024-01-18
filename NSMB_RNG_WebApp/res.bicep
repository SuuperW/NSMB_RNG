targetScope = 'resourceGroup'

param location string = resourceGroup().location
param _now string

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

resource nrCosmosDB 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
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

// Update the webapp app settings while keeping any default settings.
module appSettings 'appsettings.bicep' = {
	name: 'update-app-settings-${_now}'
	params: {
		webAppName: nrWebApp.name
		currentAppSettings: list(resourceId('Microsoft.Web/sites/config', nrWebApp.name, 'appsettings'), '2023-01-01').properties
		appSettings: {
			CosmosEndpointUri: nrCosmosDB.properties.documentEndpoint
			CosmosPrimaryKey: nrCosmosDB.listKeys().primaryMasterKey
			CosmosDbName: 'defaultDB'
			CosmosContainerName: 'defaultContainer'
		}
	}
}
