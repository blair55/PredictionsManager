'use strict';

/**
 * @ngdoc overview
 * @name frontendApp
 * @description
 * # frontendApp
 *
 * Main module of the application.
 */
angular
  .module('frontendApp', [
    'ngAnimate',
    'ngCookies',
    'ngResource',
    'ngRoute',
    'ngSanitize',
    'ngTouch',
    'LocalStorageModule',
    'ui.bootstrap.datetimepicker'
  ])
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
        templateUrl: 'views/main.html',
        controller: 'MainCtrl'
      })
      .when('/about', {
        templateUrl: 'views/about.html',
        controller: 'AboutCtrl'
      })
      .when('/leaguetable', {
        templateUrl: 'views/leaguetable.html',
        controller: 'LeaguetableCtrl'
      })
      .when('/player/:playerName', {
        templateUrl: 'views/player.html',
        controller: 'PlayerCtrl'
      })
      .when('/playergameweek/:playerName/:gameWeekNo', {
        templateUrl: 'views/playergameweek.html',
        controller: 'PlayergameweekCtrl'
      })
      .when('/admin/addgameweek', {
        templateUrl: 'views/adminaddgameweek.html',
        controller: 'AdminaddgameweekCtrl'
      })
      .when('/opengameweeks', {
        templateUrl: 'views/opengameweeks.html',
        controller: 'OpengameweeksCtrl'
      })
      .when('/openfixtures/:gameWeekNo', {
        templateUrl: 'views/openfixtures.html',
        controller: 'OpenfixturesCtrl'
      })
      .otherwise({
        redirectTo: '/'
      });
  });
