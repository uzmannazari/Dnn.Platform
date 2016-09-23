(function () {
    'use strict';

    var debugMode = window.parent['editBarSettings']['debugMode'] === true;
    var cdv = window.parent['editBarSettings']['buildNumber'];

    requirejs.config({
        baseUrl: 'scripts/contrib/',
        paths: {
            'templatePath': '../../',
            'cssPath': '../../css/'
        },
        urlArgs: (cdv ? 'cdv=' + cdv : '') + (debugMode ? '&t=' + Math.random() : '')
    });
    requirejs.onError = function (err) {
        // If requireJs throws a timeout reload the page
        if (err.requireType === 'timeout') {
            console.error(err);
            location.reload();
        }
        else {
            console.error(err);
            throw err;
        }
    };
})();

require(['jquery', 'knockout', '../util', '../sf', '../config', '../eventEmitter', '../gateway'],
    function ($, ko, ut, sf, cf, eventEmitter, Gateway) {
        var iframe = window.parent.document.getElementById("editBar-iframe");
        if (!iframe) return;

        var config = cf.init();
        var utility = ut.init(config);

        var $editBar = $("#edit-bar");

        var menuLoaders = {};

        window.requirejs.config({
            paths: {
                'rootPath': utility.getApplicationRootPath()
            }
        });

        // define util -- very important
        var util = {
            sf: sf.init(config.siteRoot, config.tabId, config.antiForgeryToken)
        };
        util = $.extend(util, utility);
        // end define util

        var getMenuLoader = function (item, callback) {
            if (!item.loader) {
                return;
            }

            if (typeof menuLoaders[item.name] !== "undefined") {
                if (typeof callback === "function") {
                    callback(menuLoaders[item.name]);
                }
                return;
            }

            var templateSuffix = '.html';
            var initMethod = 'init';
            var requiredArray = ['../' + item.loader, 'text!../../' + item.loader + templateSuffix];

            window.require(requiredArray, function (loader, html) {
                if (typeof loader === "undefined") {
                    return;
                }

                var params = { html: html };

                loader[initMethod].call(loader, item, util, params, function() {
                    menuLoaders[item.name] = loader;

                    if (typeof callback === "function") {
                        callback(loader);
                    }
                });
            });
        }

        var loadMenuStyles = function (item) {
            if (!item.loader) {
                return;
            }

            var cssSuffix = '.css';
            var requiredArray = ['css!../../css/' + item.loader + cssSuffix];

            window.require(requiredArray);
        }

        var renderMenu = function (menuItem) {
            if (menuItem.template) {
                return menuItem.template;
            } else {
                var text = menuItem.resx[menuItem.text] || menuItem.text;
                return '<button href="javascript:void(0);">' + text + '</button>' +
                    '<div class="submenuEditBar">' + text + '</div>';
            }
        }

        var menuItemClick = function (menuItem) {
            for (var name in menuLoaders) {
                if (menuLoaders.hasOwnProperty(name) && name !== menuItem.name) {
                    var loader = menuLoaders[name];
                    loader["onBlur"].call(loader);
                }
            }

            getMenuLoader(menuItem, function(loader) {
                loader["onClick"].call(loader, menuItem);
            });
        }

        var buildViewModel = function() {
            var viewModel = {};
            viewModel.leftMenus = ko.observableArray([]);
            viewModel.rightMenus = ko.observableArray([]);

            for (var i = 0; i < config.items.length; i++) {
                var menuItem = config.items[i];
                menuItem.resx = util.resx[menuItem.name] || util.resx.Common;
                loadMenuStyles(menuItem);

                switch (menuItem.parent.toLowerCase()) {
                    case "leftmenu":
                        viewModel.leftMenus.push(menuItem);
                        break;
                    case "rightmenu":
                        viewModel.rightMenus.push(menuItem);
                        break;
                }
            }

            viewModel.renderMenu = renderMenu;
            viewModel.menuItemClick = menuItemClick;

            return viewModel;
        }

        var loadMenus = function() {
            var viewModel = buildViewModel();
            ko.applyBindings(viewModel, $editBar[0]);
        }

        util.loadResx(function() {
            loadMenus();
        });
        
        // Register a PersonaBar object in the parent window global scope
        // to allow easy integration between the site and the persona bar
        window.parent.dnn.EditBar = new Gateway(util);
    });
