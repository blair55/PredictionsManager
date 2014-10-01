'use strict';

describe('Controller: PlayergameweekCtrl', function () {

  // load the controller's module
  beforeEach(module('frontendApp'));

  var PlayergameweekCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    PlayergameweekCtrl = $controller('PlayergameweekCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});
