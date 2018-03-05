// tab controller
app.controller('MenuModuleCtrl', function ($scope, $state, menuModuleService) {

    $scope.Page = 1;
    $scope.PageSize = 50;

    menuModuleService.resource.query({ Page: $scope.Page, Size: $scope.PageSize }, function (success) {
        $scope.items = success.data.Items;
    }
    , function (error) {
        console.log(error);
    });

    //获取列表数据
    //$http.post($scope.app.host + '/api/modulemenu/query', { Page: $scope.Page, Size: $scope.PageSize })
    //             .then(function (success) {
    //                 $scope.items = success.data.Items;
    //             });

    $scope.GoAdit = function (sid) {
        $state.go("sys.menumoduleadit", { sid: sid })
    }

});