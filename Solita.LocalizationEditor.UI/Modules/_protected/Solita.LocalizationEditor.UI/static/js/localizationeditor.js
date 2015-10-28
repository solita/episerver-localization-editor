"use strict";



var domUpdateHelper = (function () {
    var mod = {};

    var getOriginalValue = function (translationInput) {
        return $(translationInput).data("original");
    }

    var getDefaultValue = function (translationInput) {
        return $(translationInput).closest(".translations-row").data("default");
    }

    mod.getInputsByLanguage = function (lang) {
        return $("td[data-lang='" + lang + "']").find(".translation-input");
    }

    mod.setOriginalValue = function (translationInput) {
        $(translationInput).val(getOriginalValue(translationInput)).change();
    }

    mod.setDefaultValue = function (translationInput) {
        $(translationInput).val(getDefaultValue(translationInput)).change();
    }

    mod.setModifiedStatus = function (translationInput) {
        translationInput.removeClass("value-modified");
        translationInput.removeClass("value-new");
        var originalValue = getOriginalValue(translationInput);
        if (translationInput.val() !== originalValue) {
            if (originalValue == "") {
                translationInput.addClass("value-new");
            } else {
                translationInput.addClass("value-modified");
            }
        }
    }

    mod.setInput = function (key, language, translation) {
        $(".translations-row[data-key='" + key + "']")
            .find("td[data-lang='" + language + "']")
            .find(".translation-input")
            .val(translation)
            .change();
    }

    mod.updateLocalizationsDom = function (jsonLocalizations) {
        if (jsonLocalizations == null || jsonLocalizations === 'undefined') {
            throw "argument null in updateLocalizationsDom";
        }
        var def = Q.defer();
        Q.fcall(function () {
            jsonLocalizations.forEach(function (localization) {
                localization.Translations.forEach(function (translation) {
                    mod.setInput(localization.Key, translation.Language, translation.Value);
                });
            });
            def.resolve(null);
        });
        return def.promise;
    };

    return mod;
}());

var localizationsRepo = (function () {
    var mod = {};

    mod.ajaxCall = function (paramsObj) {
        return Q($.ajax(paramsObj));
    }

    mod.getLocalizationsForVersion = function (version) {
        var url = $(".localization-editor").data("localizations-url") + '?version=' + version;
        return mod.ajaxCall({
            url: url,
            type: 'POST',
            dataType: 'json'
        });
    };

    mod.getLocalizationsJsonFromXml = function (xmlString) {
        var attrName = "localizations-transform-url";
        var url = $(".localization-editor").data(attrName);
        if (url === null || url === undefined || url === "") { throw "Could not get url from data attribute: " + attrName }
        return mod.ajaxCall(
        {
            url: url,
            type: 'POST',
            data: xmlString,
            dataType: 'json'
        });
    };

    return mod;
}());

var languageditor = (function () {
    $(document).ready(function () {
        var domHelper = domUpdateHelper;

        var setProgressLoaderVisible = function (visible) {
            var progressLoaderSelector = "#progress-loader";
            var loader = $(progressLoaderSelector);
            if (visible) {
                loader.show();
            } else {
                loader.hide();
            }
        };

        var getFileUpload = function () {
            return $(".file-upload")[0];
        }

        var getResponseText = function (error) {
            var errorPattern = /<h2>.*<i>.*<\/i>/;
            var responseText = error.responseText;
            if (responseText === "" || responseText === undefined) {
                return error.statusText;
            }
            var match = error.responseText.match(errorPattern);
            if (match.length === 0) {
                return error.responseText;
            }
            var errorText = match[0];
            var trimmedError = errorText.substring(errorText.indexOf("<i>") + 3);
            trimmedError = trimmedError.substr(0, trimmedError.indexOf("</i>"));

            return trimmedError;
        };

        var showImportError = function (error) {
            var errorText = getResponseText(error)

            alert(errorText);
        };

        $(".csv-export").click(function () {
            var rows = new Array();

            var firstRowColumns = new Array();
            firstRowColumns.push("Key");
            firstRowColumns.push("Description");
            $(".languages-row").find('[data-lang]').each(function () {
                var lang = $(this).data("lang");
                firstRowColumns.push(lang);
            });
            rows.push(firstRowColumns.join("\t"));

            $(".category-row").each(function () {
                var category = $(this).find(".category").text();
                rows.push(category);

                $(this).nextUntil(":not(.translations-row)").each(function () {
                    var columns = new Array();
                    columns.push($(this).data("key"));
                    columns.push($(this).data("description"));

                    $(this).find('.translation-input').each(function () {
                        columns.push($(this).val());
                    });

                    rows.push(columns.join("\t"));
                });
            });
            // Base-64 encode with UTF-8 support using the method described here https://developer.mozilla.org/en-US/docs/DOM/window.btoa
            var output = btoa(unescape(encodeURIComponent(rows.join("\r\n"))));
            $(".csv-export-datalink").attr("href", 'data:text/plain;charset=UTF-8;base64,' + output);
            $(".csv-export-datalink")[0].click();
        });
        $(".csv-import").click(function () {
            var fileupload = getFileUpload();

            if (fileupload.files.length < 1) {
                return;
            }
            var file = fileupload.files[0];

            if (!file.type.match('text/plain')) {
                alert("File needs to be a XML file.");
                return;
            }

            var reader = new FileReader();
            reader.onload = (function () {
                return function (e) {
                    Q.fcall(function () {
                        setProgressLoaderVisible(true);
                        var rows = e.target.result.split(/[\r\n]/g);
                        var languages = rows[0].split("\t").slice(2);
                        var translationRows = rows.slice(1);
                        return {
                            "rows": translationRows,
                            "languages": languages
                        }
                    })
                        .then(function (csvData) {
                            csvData.rows.forEach(function (row) {
                                var columns = row.split("\t");
                                var key = columns[0];
                                var translations = columns.slice(2);

                                for (var k = 0; k < translations.length; k++) {
                                    domHelper.setInput(key, csvData.languages[k], translations[k]);
                                }
                            });
                        })
                        .finally(function () {
                            setProgressLoaderVisible(false);
                        })
                        .catch(function (error) {
                            showImportError(error);
                        });
                };
            })(file);

            reader.readAsText(file);
        });

        $(".xml-import").click(function () {
            var fileupload = getFileUpload();

            if (fileupload.files.length < 1) {
                return;
            }
            var file = fileupload.files[0];

            if (!file.type.match('text/xml')) {
                alert("File needs to be a plain text file.");
                return;
            }

            var reader = new FileReader();
            reader.onload = (function () {
                return function (e) {
                    Q.fcall(function () {
                        setProgressLoaderVisible(true);
                    })
                    .then(function () {
                        var xml = e.target.result;
                        return localizationsRepo.getLocalizationsJsonFromXml(xml);
                    })
                    .then(function (localizations) {
                        return domHelper.updateLocalizationsDom(localizations);
                    })
                    .finally(function () {
                        setProgressLoaderVisible(false);
                    })
                    .catch(function (error) {
                        showImportError(error);
                    });
                };
            })(file);

            reader.readAsText(file);
        });

        $(".version-revert").click(function () {
            var version = $(this).siblings(".version-selector:first").val();
            Q.fcall(function () {
                setProgressLoaderVisible(true);
            })
                .then(function () {
                    return localizationsRepo.getLocalizationsForVersion(version);
                })
                .then(function (locazationsJson) {
                    return domHelper.updateLocalizationsDom(locazationsJson);
                })
                .finally(function () {
                    setProgressLoaderVisible(false);
                })
                .catch(function (error) {
                    alert("Error in reverting version: " + error.message);
                });
        });

        $(".undo-cell").click(function () {
            var translationInput = $(this).siblings(".translation-input:first");
            domHelper.setOriginalValue(translationInput);
        });

        $(".undo-all").click(function () {
            $(".translation-input").each(function () {
                domHelper.setOriginalValue($(this));
            });
        });

        $(".defaults-cell").click(function () {
            var translationInput = $(this).siblings(".translation-input:first");
            domHelper.setDefaultValue(translationInput);
        });

        $(".defaults-column").click(function () {
            var lang = $(this).parent().data("lang");
            domHelper.getInputsByLanguage(lang).each(function () {
                domHelper.setDefaultValue($(this));
            });
        });

        $(".defaults-row").click(function () {
            $(this).closest(".translations-row").find(".translation-input").each(function () {
                domHelper.setDefaultValue($(this));
            });
        });

        $(".translation-input").change(function () {
            domHelper.setModifiedStatus($(this));
        });
    });
}());