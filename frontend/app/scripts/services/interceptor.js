'use strict';

/**
 * @ngdoc service
 * @name frontendApp.interceptor
 * @description
 * # interceptor
 * Factory in the frontendApp.
 */
angular.module('frontendApp')
  .factory('interceptor', function () {
      return {
          'request': function(config) {

            console.log("before:" + config.url);
            var isApiRequest = config.url.indexOf('/api') >= 0;
            
            if(isApiRequest)
            {
              var newUrl = "http://localhost:48213" + config.url;
              console.log("newUrl: " + newUrl);
              config.url = newUrl;
            }

            console.log("after:" + config.url);

            return config;
          }

         //  // optional method
         // 'requestError': function(rejection) {
         //    // do something on error
         //    if (canRecover(rejection)) {
         //      return responseOrNewPromise
         //    }
         //    return $q.reject(rejection);
         //  },



         //  // optional method
         //  'response': function(response) {
         //    // do something on success
         //    return response;
         //  },

         //  // optional method
         // 'responseError': function(rejection) {
         //    // do something on error
         //    if (canRecover(rejection)) {
         //      return responseOrNewPromise
         //    }
         //    return $q.reject(rejection);
         //  }
        };
  });

angular.module('frontendApp')
  .config(['$httpProvider', function($httpProvider) {
    $httpProvider.interceptors.push('interceptor');
}]);