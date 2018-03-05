'use strict';

/* Controllers */

angular.module('app')
  .controller('AppCtrl',
    function ($scope, $translate, $localStorage, $window, $location, $interval, $http, $cookieStore) {
        // add 'ie' classes to html
        var isIE = !!navigator.userAgent.match(/MSIE/i);
        isIE && angular.element($window.document.body).addClass('ie');
        isSmartDevice($window) && angular.element($window.document.body).addClass('smart');

        // config
        $scope.app = {
            name: 'BossWell',
            version: '1.0.1',
            host: 'http://localhost:1431/',
            // for chart colors
            color: {
                primary: '#7266ba',
                info: '#23b7e5',
                success: '#27c24c',
                warning: '#fad733',
                danger: '#f05050',
                light: '#e8eff0',
                dark: '#3a3f51',
                black: '#1c2b36'
            },
            settings: {
                themeID: 1,
                navbarHeaderColor: 'bg-black',
                navbarCollapseColor: 'bg-white-only',
                asideColor: 'bg-black',
                headerFixed: true,
                asideFixed: false,
                asideFolded: false,
                asideDock: false,
                container: false
            }
        }

        // save settings to local storage
        if (angular.isDefined($localStorage.settings)) {
            $scope.app.settings = $localStorage.settings;
        } else {
            $localStorage.settings = $scope.app.settings;
        }
        $scope.$watch('app.settings', function () {
            if ($scope.app.settings.asideDock && $scope.app.settings.asideFixed) {
                // aside dock and fixed must set the header fixed.
                $scope.app.settings.headerFixed = true;
            }
            // save to local storage
            $localStorage.settings = $scope.app.settings;
        }, true);

        // angular translate
        $scope.lang = { isopen: false };
        $scope.langs = { en: 'English', de_DE: 'German', it_IT: 'Italian' };
        $scope.selectLang = $scope.langs[$translate.proposedLanguage()] || "English";
        $scope.setLang = function (langKey, $event) {
            // set the current lang
            $scope.selectLang = $scope.langs[langKey];
            // You can change the language during runtime
            $translate.use(langKey);
            $scope.lang.isopen = !$scope.lang.isopen;
        };

        function isSmartDevice($window) {
            // Adapted from http://www.detectmobilebrowsers.com
            var ua = $window['navigator']['userAgent'] || $window['navigator']['vendor'] || $window['opera'];
            // Checks for iOs, Android, Blackberry, Opera Mini, and Windows mobile devices
            return (/iPhone|iPod|iPad|Silk|Android|BlackBerry|Opera Mini|IEMobile/).test(ua);
        }

        //cookie 失效跳转登录页面
        var data = $cookieStore.get('AD_User');
        if (data == undefined || data == null) {
            $location.url("/login");
            return;
        }
        //用户基本信息
        $scope.UserInfo = data;


        //心跳效验登录
        $interval(
          function () {
              //cookie 失效跳转登录页面
              var data = $cookieStore.get('AD_User');
              if (data == undefined || data == null) {
                  $location.url("/login");
                  return;
              }
              var token = data.Token;
              $http.get($scope.app.host + '/api/adminuser/checkonline?token=' + token)
                  .then(function (success) {
                      var result = success.data;
                      //登录失效
                      if (result.code != 200) {
                          layer.msg(result.msg, { time: 1000 });
                          $location.url("/login");
                      }
                  });
          },
          10000
        );

        //退出登录
        $scope.loginout = function () {
            //清除cookie
            $cookieStore.remove("AD_User");
            $location.url("/login");
        }

    });