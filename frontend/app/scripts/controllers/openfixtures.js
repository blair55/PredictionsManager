'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:OpenfixturesCtrl
 * @description
 * # OpenfixturesCtrl
 * Controller of the frontendApp
 */
angular.module('frontendApp')
  .controller('OpenfixturesCtrl', function ($scope, $http, $routeParams) {
	var url = '/api/openfixtures/' + $routeParams.gameWeekNo;
    $http.get(url).success(function(data){
    	$scope.model = data;
    });
  });