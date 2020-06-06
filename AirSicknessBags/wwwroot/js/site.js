// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function ClearOutFields(fields) {
    for (var i = 0; i < fields.length; i++) {
        if (document.getElementById(fields[i]).value.includes('Part or all of bag')) {
            document.getElementById(fields[i]).value = '';
        }
    }
}

function fillNavigationAttributes(ids, value) {
    for (var i = 0; i < ids.length; i++) {
        var element = document.getElementById(ids[i]);
        if (element != null) {
        //    alert(value);
            element.setAttribute('perpage', value);
        //    alert(document.getElementById(ids[i]).getAttribute("perpage"));
        }
    }
}

// This copies over the number of items per page to the URL submitted back to the Controller Action
$(function () {
    $(".sortme").click(function () {
        // example of  modifying href 
        $(this).attr("href", $(this).attr("href") + "&perpage=" + $("#PerPage").val());
    })
})

function ToggleCheckbox(element) {
    document.getElementById(element).value = 1 - document.getElementById(element).value
}
