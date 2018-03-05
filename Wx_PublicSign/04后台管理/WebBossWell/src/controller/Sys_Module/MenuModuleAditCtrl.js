// tab controller
app.controller('MenuModuleAditCtrl', function ($scope, $state, $http) {
    $scope.sid = $state.params.sid;

    //获取父级菜单值
    $http.get($scope.app.host + '/api/modulemenu/queryparent')
                 .then(function (success) {
                     console.log(success);
                     $scope.ParentMenuJson = success.data.data;
                 });
    //保存模型
    $scope.Save = function () {

        $http.post($scope.app.host + '/api/modulemenu/save', $scope.Model)
                .then(function (success) {
                    var result = success.data;
                    if (result.code == 200) {
                        $state.go(-1);
                    }
                    else {
                        layer.msg(result.msg, { time: 1000 });
                    }
                });
    }

});