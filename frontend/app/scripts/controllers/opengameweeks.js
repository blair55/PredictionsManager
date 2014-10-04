'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:OpengameweeksCtrl
 * @description
 * # OpengameweeksCtrl
 * Controller of the frontendApp
 */
angular.module('frontendApp')
  .controller('OpengameweeksCtrl', function ($scope, $http) {
    $http.get('/api/opengameweeks').success(function(data){
    	$scope.model = data;
    });
  });
