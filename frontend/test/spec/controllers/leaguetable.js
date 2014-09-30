'use strict';

describe('Controller: LeaguetableCtrl', function () {

  // load the controller's module
  beforeEach(module('frontendApp'));

  var LeaguetableCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    LeaguetableCtrl = $controller('LeaguetableCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});