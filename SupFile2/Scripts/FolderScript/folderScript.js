function addFolder() {

    $.ajax({
        type: "GET",
        url: '/Content/addFolderModalView.html',
        success: function (msg) {
            $("#getModal").empty();
            $("#getModal").html(msg);
            $("#ptFolderModal").modal("show");
            $("#ptFolderModal form").on('submit', function (e) { validateAddFolder(e) });
        }
    })
}
function validateAddFolder(e) {
    e.preventDefault();
    var foldername = $("#ptFolderModal form input[name=folderName]").val();
    console.log(foldername);
    $.ajax({
        type: "GET",
        url: '/Folder/addFolder?foldername=' + foldername,
        success: function (msg) {
            if (msg === 'ok') {
                location.reload();
            }
            else {
                notify("danger", "dossier existe déjà");
            }
        }
    })
}
function renameFolder(folderOldName) {
    $.ajax({
        type: "GET",
        url: '/Content/renameFolderModalView.html',
        success: function (msg) {
            $("#getModal").empty();
            $("#getModal").html(msg);
            $("#rnFolderModal").modal("show");
            $("#rnFolderModal form").on('submit', function (e) { validateRenameFolder(e, folderOldName) });
        }
    })
}
function validateRenameFolder(e, folderOldName) {
    e.preventDefault();
    var foldername = $("#rnFolderModal form input[name=folderName]").val();
    $.ajax({
        type: "GET",
        url: '/Folder/Rename?foldername=' + foldername + '&folderoldname=' + folderOldName,
        success: function (msg) {
            if (msg === 'ok') {
                location.reload();
            }
            else {
                notify("danger", "nouveau name existe déjà");
            }
        }
    })
}



function addFile() {
    $.ajax({
        type: "GET",
        url: '/Content/addFileModalView.html',
        success: function (msg) {
            $("#getModal").empty();
            $("#getModal").html(msg);
            $("#ptFileModal").modal("show");
            $("#ptFileModal form").on('submit', function (e) { validateAddFile(e) });
        }
    })
}

function validateAddFile(e) {
    e.preventDefault();
    var formdata = new FormData();
    var fileInput = document.getElementById('fileName');
    for (i = 0; i < fileInput.files.length; i++) {
        formdata.append(fileInput.files[i].name, fileInput.files[i]);
    }

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/File/AddFile');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            if (responseText == 'ok') {
                location.reload();
            }
            else {
                notify("danger", "eurreur l'hors de la creation");
            }
            
        }
    }
}
function renameFile(fileOldName) {
    $.ajax({
        type: "GET",
        url: '/Content/renameFileModalView.html',
        success: function (msg) {
            $("#getModal").empty();
            $("#getModal").html(msg);
            $("#rnFileModal").modal("show");
            $("#rnFileModal form").on('submit', function (e) { validateRenameFile(e, fileOldName) });
        }
    })
}
function validateRenameFile(e, fileOldName) {
    e.preventDefault();
    var filename = $("#rnFileModal form input[name=fileName]").val();
    $.ajax({
        type: "GET",
        url: '/File/Rename?filename=' + filename + '&fileoldname=' + fileOldName,
        success: function (msg) {
            if (msg === 'ok') {
                location.reload();
            }
            else {
                notify("danger", "file name existe déjà");
            }
        }
    })
}
