// Write your Javascript code.

var app = angular.module('nsds', []);

app.config(['$compileProvider', function ($compileProvider) {
	$compileProvider.debugInfoEnabled(false);
}]);

app.controller('DeploymentController', function DeploymentController($scope) {
	$scope.selectedItems = [];
	$scope.canDeploy = false;
});

app.controller('PoolsController', function PoolsController($scope, $http) {
	$http.get('api/pools').then(function (response) {
		$scope.pools = response.data;
	});
});

app.controller('ClientsController', function ClientsController($scope, $http) {
	$http.get('api/clients/').then(function (response) {
		$scope.pools = _.groupBy(response.data, function (x) { return x.poolId; });
		$scope.clients = _.each(response.data, function (x) {
			_.each(x.modules, function (y) {
				y.selected = false;
				y.select = function () {
					y.selected = !y.selected;
					if (y.selected) {
						$scope.$parent.$parent.selectedItems.push({ client: x, module: y });
					} else {
						$scope.$parent.$parent.selectedItems = _.reject($scope.$parent.selectedItems, function (z) { return z.client == x && z.module == y; });
					}
					$scope.$parent.$parent.canDeploy = $scope.$parent.$parent.selectedItems.length > 0;
				};
			});
			x.select = function () {
				for (var i = 0; this.modules && i < this.modules.length; i++) {
					if (!this.modules[i].isLatest) {
						this.modules[i].selected = true;
					}
				}
			};
			x.selected = function () {
				return _.some(x.modules, function (y) {
					return y.selected;
				})
			};
			x.status = function () {
				return "ok";
			};
		});
		$scope.deploy = function (client, module) {

		};
	});
});

app.controller('PackagesController', function PackagesController($scope, $http) {
	$scope.deploy = function (package) {
//		console.log($scope);
		package.isDeploying = true;
		var parent = document.getElementById(package.$$hashKey.replace(':', '_'));
		var spinner = new Spinner({
			lines: 9 // The number of lines to draw
			, length: 5 // The length of each line
			, width: 3 // The line thickness
			, radius: 5 // The radius of the inner circle
			, scale: 1 // Scales overall size of the spinner
			, corners: 1 // Corner roundness (0..1)
			, color: '#ccc' // #rgb or #rrggbb or array of colors
			, opacity: 0.05 // Opacity of the lines
			, rotate: 0 // The rotation offset
			, direction: 1 // 1: clockwise, -1: counterclockwise
			, speed: 0.7 // Rounds per second
			, trail: 75 // Afterglow percentage
			, fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
			, zIndex: 2e9 // The z-index (defaults to 2000000000)
			, className: 'spinner' // The CSS class to assign to the spinner
			, top: '50%' // Top position relative to parent
			, left: '50%' // Left position relative to parent
			, shadow: false // Whether to render a shadow
			, hwaccel: false // Whether to use hardware acceleration
			, position: 'relative' // Element positioning
		}).spin(parent);
		$http.get('/api/deploy/package/' + package.name).then(function (response) {
//			console.log(response.data);
			package.isDeploying = false;
			spinner.stop();
		});
	};
	$http.get('api/packages').then(function (response) {
		$scope.packages = _.each(response.data, function (x) {
			x.isDeploying = false;
		});
	});
});
