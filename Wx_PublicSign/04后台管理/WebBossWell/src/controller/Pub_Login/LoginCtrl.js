app.controller('LoginCtrl', function ($scope, $state, $http, $resource, $cookieStore) {

    //登录
    $scope.login = function () {

        //var authdata = Base64.encode($scope.user.email + ':' + $scope.user.password);

        $http.post($scope.app.host + '/api/adminuser/login', { account: $scope.user.email, password: $scope.user.password }).then(function (success) {
            var result = success.data;

            if (result.code == 200) {
                var expireDate = new Date();
                expireDate.setDate(expireDate.getDate() + 1);//设置cookie保存1天
                $cookieStore.put('AD_User', result.data, { 'expires': expireDate });
                $state.go("home.profile");
            }
            else {
                layer.msg(result.msg, { time: 1000 });
            }
        });

    }

});