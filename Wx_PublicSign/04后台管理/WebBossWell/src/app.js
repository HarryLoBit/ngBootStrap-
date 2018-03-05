'use strict';
var app = angular.module('app', [
    'ngAnimate',
    'ngCookies',
    'ngResource',
    'ngSanitize',
    'ngTouch',
    'ngStorage',
    'ui.router',
    'ui.bootstrap',
    'ui.load',
    'ui.jq',
    'ui.validate',
    'oc.lazyLoad',
    'pascalprecht.translate'
]);


app.run(function ($rootScope, $state, $stateParams) {
          $rootScope.$state = $state;
          $rootScope.$stateParams = $stateParams;
      }
  )


//服务器返回401
app.factory('AuthInterceptor', function ($rootScope, $q, $location) {
    return {
        responseError: function (response) {
            if (response.status == 401) {
                $location.url('/login');
            }
            return $q.reject(response);
        }
    };
})


//路由配置
app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

    //默认路由
    $urlRouterProvider
             .otherwise('/home/profile');
    $stateProvider

        //<!--权限配置路由-->
        .state('sys', {
            abstract: true,
            url: '/sys',
            templateUrl: 'statichtm/app.html',
        })
        //菜单模块
        .state('sys.menumodule', {
            url: '/menumodule',
            templateUrl: 'statichtm/Sys_Module/MenuModule.html',
        })

         //菜单模块(编辑)
        .state('sys.menumoduleadit', {
            url: '/menumoduleadit/:sid',
            templateUrl: 'statichtm/Sys_Module/MenuModuleAdit.html',
        })

        //后台管理员
        .state('sys.adminuser', {
            url: '/adminuser',
            templateUrl: 'statichtm/Sys_Admin/AdminUser.html',
        })
        //系统角色
        .state('sys.role', {
            url: '/role',
            templateUrl: 'statichtm/Sys_Role/Role.html',
        })
        //维护角色权限
        .state('sys.roleauthor', {
            url: '/roleauthor',
            templateUrl: 'statichtm/Sys_RoleAuthor/RoleAuthor.html',
        })

        //<!--系统消息路由-->
         .state('msg', {
             abstract: true,
             url: '/msg',
             templateUrl: 'statichtm/app.html',
         })
        //系统日志
        .state('msg.logs', {
            url: '/logs',
            templateUrl: 'statichtm/Pub_Log/Logs.html',
        })

        //<!--全局级路由-->
        .state('home', {
            abstract: true,
            url: '/home',
            templateUrl: 'statichtm/app.html',
        })
        //主页面
        .state('home.profile', {
            url: '/profile',
            templateUrl: 'statichtm/Home/Profile.html',
        })
        //登录页
        .state('login', {
            url: '/login',
            templateUrl: 'statichtm/temp/page_signin.html',
        })
}]);