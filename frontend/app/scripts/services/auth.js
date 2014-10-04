'use strict';

/**
 * @ngdoc service
 * @name frontendApp.auth
 * @description
 * # auth
 * Service in the frontendApp.
 */
angular.module('frontendApp')
  .service('auth', function auth($http, localStorageService) {
  	var withPlayer = function(callback){

		var localPlayer = localStorageService.get('player');

		if (localPlayer) {
			callback(localPlayer);
		}
		else {
			$http.get('/api/whoami').success(function(player){
				localStorageService.set('player', player);
				callback(player);
			});
		}

  	};

  	return { withPlayer : withPlayer };
  });
