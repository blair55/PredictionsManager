'use strict';

/**
 * @ngdoc service
 * @name frontendApp.auth
 * @description
 * # auth
 * Service in the frontendApp.
 */
angular.module('frontendApp')
  .service('auth', function auth($http) {
  	var withPlayer = function(callback){
		$http.get('/api/whoami').success(callback);
  	};
  	return { withPlayer : withPlayer };
  });
