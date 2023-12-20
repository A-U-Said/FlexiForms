angular.module("umbraco").controller("FlexiForms.DeleteController", function ($scope, FlexiFormsResponsesResource, treeService, navigationService) {

	var vm = this;

	vm.performDelete = function() {
	
		//mark it for deletion (used in the UI)
		$scope.currentNode.loading = true;
			
		FlexiFormsResponsesResource.DeleteAllByIdentifier($scope.currentNode.name)
			.then(function() {
				$scope.currentNode.loading = false;
				var rootNode = treeService.getTreeRoot($scope.currentNode);
				treeService.removeNode($scope.currentNode);
				navigationService.hideMenu();
			});
	};

	vm.cancel = function() {
		navigationService.hideDialog();
	};
		
});