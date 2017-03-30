// Write your Javascript code.

var app = angular.module('nsds', []);

app.controller('PoolsController', function PoolsController($scope, $http) {
	$http.get('api/pools').then(function (response) {
		$scope.pools = response.data;
	});
});

app.controller('ClientsController', function ClientsController($scope, $http) {
	$http.get('api/clients/').then(function (response) {
		$scope.pools = _.groupBy(response.data, function (x) { return x.poolId; });
		$scope.clients = _.each(response.data, function (x) {
			x.status = function () {
				return "ok";
			}
		});
	});
});

app.controller('PackagesController', function PackagesController($scope, $http) {
	$http.get('api/packages').then(function (response) {
		$scope.packages = response.data;
	});
});
