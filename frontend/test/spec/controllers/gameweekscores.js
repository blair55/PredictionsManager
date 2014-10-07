'use strict';

describe('Controller: GameweekscoresCtrl', function () {

  // load the controller's module
  beforeEach(module('frontendApp'));

  var GameweekscoresCtrl,
    scope;

  // Initialize the controller and a mock scope
  beforeEach(inject(function ($controller, $rootScope) {
    scope = $rootScope.$new();
    GameweekscoresCtrl = $controller('GameweekscoresCtrl', {
      $scope: scope
    });
  }));

  it('should attach a list of awesomeThings to the scope', function () {
    expect(scope.awesomeThings.length).toBe(3);
  });
});
