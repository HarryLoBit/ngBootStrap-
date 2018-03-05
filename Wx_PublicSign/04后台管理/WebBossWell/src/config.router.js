'use strict';

/**
 * Config for the router
 */
angular.module('app')
  .run(
    ['$rootScope', '$state', '$stateParams',
      function ($rootScope, $state, $stateParams) {
          $rootScope.$state = $state;
          $rootScope.$stateParams = $stateParams;
      }
    ]
  )
  .config(
    ['$stateProvider', '$urlRouterProvider', '$routeProvider',
      function ($stateProvider, $urlRouterProvider, $routeProvider) {
          //$urlRouterProvider
          //    .otherwise('/app/profile');
          //$stateProvider
          //    .state('app', {
          //        abstract: true,
          //        url: '/app',
          //        templateUrl: 'tpl/app.html',
          //    })
          //    .state('app.profile', {
          //        url: '/profile',
          //        templateUrl: 'tpl/page_profile.html',
          //    })

          //首页
          $routeProvider.when('/app', {
              templateUrl: "tpl/app.html",
              controller: 'AppCtrl'
          });
          //主页面
          $routeProvider.when('/profile', {
              templateUrl: "tpl/page_profile.html",
              controller: 'ProfileCtrl'
          });

          //菜单模块
          $routeProvider.when('/menumodule', {
              templateUrl: "tpl/System/sys_menumodule.html",
              controller: 'MenuModuleCtrl'
          });

          $routeProvider.otherwise({
              redirectTo: '/app'
          });

      }
    ]
  );