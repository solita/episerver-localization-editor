"use strict";

var languageditor = (function() {
    $(document).ready(function() {

        $(".csv-export").click(function() {
            var rows = new Array();

            var firstRowColumns = new Array();
            firstRowColumns.push("Key");
            firstRowColumns.push("Description");
            $(".languages-row").find('[data-lang]').each(function() {
                var lang = $(this).data("lang");
                firstRowColumns.push(lang);
            });
            rows.push(firstRowColumns.join("\t"));

            $(".category-row").each(function() {
                var category = $(this).find(".category").text();
                rows.push(category);

                $(this).nextUntil(":not(.translations-row)").each(function() {
                    var columns = new Array();
                    columns.push($(this).data("key"));
                    columns.push($(this).data("description"));

                    $(this).find('.translation-input').each(function() {
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

        $(".csv-import").click(function() {
            var fileupload = $(".csv-file-upload")[0];

            if (fileupload.files.length < 1) {
                return;
            }

            var file = fileupload.files[0];

            if (!file.type.match('text/plain')) {
                alert("File needs to be a plain text file.");
                return;
            }

            var reader = new FileReader();
            reader.onload = (function() {
                return function(e) {
                    var rows = e.target.result.split(/[\r\n]/g);
                    var languages = rows[0].split("\t").slice(2);
                    var translationRows = rows.slice(1);

                    translationRows.forEach(function(row) {
                        var columns = row.split("\t");
                        var key = columns[0];
                        var translations = columns.slice(2);

                        for (var k = 0; k < translations.length; k++) {
                            setInput(key, languages[k], translations[k]);
                        }
                    });
                };
            })(file);

            reader.readAsText(file);
        });

        $(".version-revert").click(function() {
            var version = $(this).siblings(".version-selector:first").val();
            var url = $(".language-editor").data("localizations-url") + '?version=' + version;

            $.ajax({
                url: url,
                success: function(json) {
                    json.localizations.forEach(function(localization) {
                        localization.translations.forEach(function(translation) {
                            setInput(localization.key, translation.language, translation.value);
                        });
                    });
                },
                type: 'POST',
                dataType: 'json'
            });
        });

        $(".undo-cell").click(function() {
            var translationInput = $(this).siblings(".translation-input:first");
            setOriginalValue(translationInput);
        });

        $(".undo-all").click(function() {
            $(".translation-input").each(function() {
                setOriginalValue($(this));
            });
        });

        $(".defaults-cell").click(function() {
            var translationInput = $(this).siblings(".translation-input:first");
            setDefaultValue(translationInput);
        });

        $(".defaults-column").click(function() {
            var lang = $(this).parent().data("lang");
            getInputsByLanguage(lang).each(function() {
                setDefaultValue($(this));
            });
        });

        $(".defaults-row").click(function() {
            $(this).closest(".translations-row").find(".translation-input").each(function() {
                setDefaultValue($(this));
            });
        });

        $(".translation-input").change(function() {
            setModifiedStatus($(this));
        });
    });

    function getInputsByLanguage(lang) {
        return $("td[data-lang='" + lang + "']").find(".translation-input");
    }

    function setOriginalValue(translationInput) {
        $(translationInput).val(getOriginalValue(translationInput)).change();
    }

    function getOriginalValue(translationInput) {
        return $(translationInput).data("original");
    }

    function setDefaultValue(translationInput) {
        $(translationInput).val(getDefaultValue(translationInput)).change();
    }

    function getDefaultValue(translationInput) {
        return $(translationInput).closest(".translations-row").data("default");
    }

    function setModifiedStatus(translationInput) {
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

    function setInput(key, language, translation) {
        $(".translations-row[data-key='" + key + "']")
            .find("td[data-lang='" + language + "']")
            .find(".translation-input")
            .val(translation)
            .change();
    }
    
}());