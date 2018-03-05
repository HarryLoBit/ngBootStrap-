angular.module('app').factory('menuModuleService', ['$resource', menuModuleService]);
function menuModuleService($resource) {
    var service = {};
    service.resource = $resource($scope.app.host + '/api/modulemenu/:id', { id: '@id' },
                                     {
                                         query: { method: 'POST', url: apiserver.Url + "/api/modulemenu/query" }
                                     }
                                 );
    return service;
}