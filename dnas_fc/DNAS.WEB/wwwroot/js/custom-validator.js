let allowedSpecialCharacter;

$(document).ready(function () {
    $.getJSON('/Login/GetRegexPatternForClientSideValidation', function (data) {
        debugger;
        allowedSpecialCharacter = new RegExp(data);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error("Error fetching regex:", textStatus, errorThrown);
    });

});


// 1. Register custom validation method
$.validator.addMethod("nospecialcharacter", function (value, element) {
    debugger;
    return this.optional(element) || allowedSpecialCharacter.test(value);
}, "Special characters are not allowed!");

// 2. Register rule for Unobtrusive Validation
$.validator.unobtrusive.adapters.add("nospecialcharacter", [], function (options) {
    options.rules["nospecialcharacter"] = true;
    options.messages["nospecialcharacter"] = options.message;
});

// 3. Ensure validation works on page load
$(function () {
    $("form").each(function () {
        $(this).validate();
    });
});