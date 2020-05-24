function confirmDelete(uniqueID) {
    var deleteSpan = 'deleteSpan_' + uniqueID;
    var confirmSpan = 'confirmSpan_' + uniqueID;

        $('#' + deleteSpan).toggle();
        $('#' + confirmSpan).toggle();
}

