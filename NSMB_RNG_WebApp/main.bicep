param location string = 'eastus'
param _now string = utcNow()

targetScope = 'subscription'
resource resGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
	location: location
	name: 'NSMB_RNG'
}

module resModule 'res.bicep' = {
	name: 'ngResources-${_now}' 
	scope: resGroup
	params: {
		location: resGroup.location
		_now: _now
	}
}
