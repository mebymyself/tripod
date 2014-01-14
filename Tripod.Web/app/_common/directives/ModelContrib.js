'use strict';
var App;
(function (App) {
    (function (Directives) {
        (function (ModelContrib) {
            ModelContrib.directiveName = 'modelContrib';

            var Controller = (function () {
                function Controller(scope) {
                    this.hasError = false;
                    this.hasSuccess = false;
                    this.hasSpinner = false;
                    this.serverError = '';
                }
                Controller.prototype.spinnerCssClass = function () {
                    return this.hasSpinner ? 'has-spinner' : null;
                };

                Controller.prototype.errorCssClass = function () {
                    return this.hasError ? 'has-error' : null;
                };

                Controller.prototype.successCssClass = function () {
                    return this.hasSuccess ? 'has-success' : null;
                };

                Controller.prototype.hasFeedback = function () {
                    return this.hasError || this.hasSuccess || this.hasSpinner;
                };

                Controller.prototype.feedbackCssClass = function () {
                    if (this.hasSpinner)
                        return this.spinnerCssClass();
                    if (this.hasError)
                        return this.errorCssClass();
                    if (this.hasSuccess)
                        return this.successCssClass();
                    return null;
                };

                Controller.prototype.inputGroupCssClass = function (size) {
                    if (!this.hasFeedback())
                        return null;
                    var cssClass = 'input-group';
                    if (size)
                        cssClass += ' input-group-' + size;
                    return cssClass;
                };
                Controller.$inject = ['$scope'];
                return Controller;
            })();
            ModelContrib.Controller = Controller;

            var directiveFactory = function () {
                return function () {
                    var directive = {
                        name: ModelContrib.directiveName,
                        restrict: 'A',
                        require: [ModelContrib.directiveName, 'ngModel', '^formContrib'],
                        controller: Controller,
                        link: function (scope, element, attr, ctrls) {
                            var modelContribCtrl = ctrls[0];
                            var modelCtrl = ctrls[1];
                            var formContribCtrl = ctrls[2];

                            var alias = $.trim(attr['name']);
                            if (alias)
                                formContribCtrl[alias] = modelContribCtrl;

                            scope.$watch(function () {
                                return [modelCtrl.$valid, modelCtrl.$dirty, formContribCtrl.isSubmitAttempted, modelContribCtrl.hasSpinner];
                            }, function () {
                                var isDirtyOrSubmitAttempted = modelCtrl.$dirty || formContribCtrl.isSubmitAttempted;
                                modelContribCtrl.hasError = !modelContribCtrl.hasSpinner && modelCtrl.$invalid && isDirtyOrSubmitAttempted;
                                modelContribCtrl.hasSuccess = !modelContribCtrl.hasSpinner && modelCtrl.$valid && isDirtyOrSubmitAttempted;
                            }, true);
                        }
                    };
                    return directive;
                };
            };

            ModelContrib.directive = directiveFactory();
        })(Directives.ModelContrib || (Directives.ModelContrib = {}));
        var ModelContrib = Directives.ModelContrib;
    })(App.Directives || (App.Directives = {}));
    var Directives = App.Directives;
})(App || (App = {}));