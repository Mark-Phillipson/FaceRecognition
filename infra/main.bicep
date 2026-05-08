// Bicep template to create an Azure Static Web App (Free tier) in UK South
@description('Main Bicep file to provision a Static Web App')

param environmentName string = 'prod'
param location string = 'uksouth'
param skuName string = 'Free'
param appLocation string = 'FaceRecognition.Client'
param appArtifactLocation string = 'wwwroot'

var resourceToken = uniqueString(subscription().id, resourceGroup().id, location, environmentName)
var staticSiteName = 'azswa${resourceToken}'

resource staticSite 'Microsoft.Web/staticSites@2024-11-01' = {
  name: staticSiteName
  location: location
  sku: {
    name: skuName
  }
  properties: {
    buildProperties: {
      appLocation: appLocation
      apiLocation: ''
      appArtifactLocation: appArtifactLocation
    }
  }
  tags: {
    environment: environmentName
  }
}

output staticSiteName string = staticSite.name
output staticSiteDefaultHostname string = staticSite.properties.defaultHostname
