'use strict';
define(["require", "exports"], function(require, exports) {
    exports.directiveName = 'serverError';

    //#region Directive
    var directiveFactory = function () {
        return function () {
            var directive = {
                name: exports.directiveName,
                restrict: 'A',
                require: ['ngModel', 'modelHelper'],
                link: function (scope, element, attr, ctrls) {
                    // don't initialize this unless there is a value in the attribute
                    var serverError = attr['serverError'];
                    if (!serverError)
                        return;

                    // passwords may not be valid on server, but will come back with empty box
                    var inputType = attr['type'];
                    var isPassword = inputType && inputType.toLowerCase() == 'password';

                    var modelCtrl = ctrls[0];
                    var helpCtrl = ctrls[1];

                    // set the server error text on the model helper controller
                    helpCtrl.serverError = serverError;

                    // initial watch to remove required error and set server error
                    var initialValue;
                    var initWatch = scope.$watch(function () {
                        return modelCtrl.$error;
                    }, function (value) {
                        initialValue = modelCtrl.$viewValue;
                        if (value.required && isPassword) {
                            modelCtrl.$setValidity('required', true);
                        }
                        modelCtrl.$setValidity('server', false);
                        initWatch(); // remove this watch now
                    });

                    // watch for changes and hide / show server error accordingly
                    scope.$watch(function () {
                        return modelCtrl.$viewValue;
                    }, function (value) {
                        // always remove the error message when input becomes dirty
                        if (modelCtrl.$dirty) {
                            modelCtrl.$setValidity('server', true);
                        }

                        // restore the error message when input becomes pristine
                        // unless it is a password
                        if (!isPassword && value === initialValue) {
                            modelCtrl.$setValidity('server', false);
                        }
                    });
                }
            };
            return directive;
        };
    };

    //#endregion
    exports.directive = directiveFactory();
});