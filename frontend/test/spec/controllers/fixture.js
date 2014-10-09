'use strict';

describe('Controller: FixtureCtrl', function () {

  // load the controller's module
  beforeEach(module('frontendApp'));

  var FixtureCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    FixtureCtrl = $controller('FixtureCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});
