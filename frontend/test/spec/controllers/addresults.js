'use strict';

describe('Controller: AddresultsCtrl', function () {

  // load the controller's module
  beforeEach(module('frontendApp'));

  var AddresultsCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    AddresultsCtrl = $controller('AddresultsCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});
