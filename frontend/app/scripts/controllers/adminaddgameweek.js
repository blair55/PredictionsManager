'use strict';

/**
 * @ngdoc function
 * @name frontendApp.controller:AdminaddgameweekCtrl
 * @description
 * # AdminaddgameweekCtrl
 * Controller of the frontendApp
 */
angular.module('frontendApp')
  .controller('AdminaddgameweekCtrl', function ($scope, $http) {

  	$scope.hours = [12,13,14,15,16,17,18,19,20];
  	$scope.minutes = [];

  	for(var i=0; i<60; i++){
  		if(i%5==0) $scope.minutes.push(i);
  	}

    $scope.gameweek = {
		fixtures: [
			{ home:"", away:"", kickoff: { date:"", hour:12, min:45 } },
			{ home:"", away:"", kickoff: { date:"", hour:15, min:0 } },
			{ home:"", away:"", kickoff: { date:"", hour:15, min:0 } }
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
	    	//$scope.gameweek.number = data;
	    });
    };

  });
