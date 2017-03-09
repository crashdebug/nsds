// Write your Javascript code.

var app = angular.module('nsds', []);

app.controller('PoolsController', function PoolsController($scope, $http) {
	$http.get('api/pools').then(function (response) {
		$scope.pools = response.data;
	});
});

app.controller('ClientsController', function ClientsController($scope, $http) {
	$http.get('api/clients/pool/' + $scope.$parent.pool.id).then(function (response) {
		$scope.clients = _.each(response.data, function (x) {
			x.status = function () {
				return "ok";
			}
		});
	});
});
