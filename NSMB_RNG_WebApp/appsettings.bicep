// I took this from https://stackoverflow.com/questions/72940236/is-there-a-workaround-to-keep-app-settings-which-not-defined-in-bicep-template
param webAppName string
param appSettings object
param currentAppSettings object

resource webApp 'Microsoft.Web/sites@2023-01-01' existing = {
  name: webAppName
}

resource siteconfig 'Microsoft.Web/sites/config@2023-01-01' = {
  parent: webApp
  name: 'appsettings'
  properties: union(currentAppSettings, appSettings)
}
