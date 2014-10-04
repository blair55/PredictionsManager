'use strict';

describe('Controller: AdminaddgameweekCtrl', function () {

  // load the controller's module
  beforeEach(module('frontendApp'));

  var AdminaddgameweekCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    AdminaddgameweekCtrl = $controller('AdminaddgameweekCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});
