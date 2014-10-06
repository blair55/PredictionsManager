'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:AdminaddgameweekCtrl
 * @description
 * # AdminaddgameweekCtrl
 * Controller of the frontendApp
 */
angular.module('frontendApp')
  .controller('AdminaddgameweekCtrl', function ($scope, $http, $location, localStorageService, notify) {

    $scope.gameweek = {
    	fixtures: [
    		{ home:"", away:"", kickoff: new Date() },
    		{ home:"", away:"", kickoff: new Date() },
    		{ home:"", away:"", kickoff: new Date() }
    	]
    };

    $http.get('/api/admin/getnewgameweekno').success(function(data){
    	$scope.gameweek.number = data;
    });

    $scope.addFixture = function(){
  	var latestFixture = $scope.gameweek.fixtures[$scope.gameweek.fixtures.length - 1];
  	var newKickOff = angular.copy(latestFixture.kickoff);
    	$scope.gameweek.fixtures.push({home:"", away:"", kickoff:newKickOff});
    }

    $scope.removeFixture = function(index){
    	$scope.gameweek.fixtures.splice( index, 1 );
    }

    $scope.submit = function(){
      $http.post('/api/admin/gameweek', $scope.gameweek).success(function(data){
        notify.success('gameweek added')
        $location.path('openfixtures');
      });
    };

  });