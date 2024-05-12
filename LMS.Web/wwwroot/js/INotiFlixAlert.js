function ShowAlert(msg, ErrorCode) {
    var title = "";
    var buttontext = "OK";
    if (ErrorCode == "200") {
        msgtype = "200";
        Notiflix.Notify.success(           
            msg          
        );
    }
    else if (ErrorCode == "400") {       
        Notiflix.Notify.failure(
            msg            
        );
    }
    else if (ErrorCode == "404") {       
        Notiflix.Notify.warning(
            msg           
        );
    }
    else {       
        Notiflix.Notify.info(
            msg          
        );
    }    
}
function ShowAlertandRedirect(msg, ErrorCode, Url) {
    var title = "";
    var buttontext = "OK";
    if (ErrorCode == "200") {
        msgtype = "200";
        Notiflix.Report.success(
            title,
            msg,
            buttontext,
            function cb() {
                document.location.href = Url;
            },
        );
    }
    else if (ErrorCode == "400") {
        Notiflix.Report.failure(
            title,
            msg,
            buttontext,
            function cb() {
                document.location.href = Url;
            },
        );
    }
    else if (ErrorCode == "404") {
        Notiflix.Report.warning(
            title,
            msg,
            buttontext,
            function cb() {
                document.location.href = Url;
            },
        );
    }
    else {
        Notiflix.Report.info(
            title,
            msg,
            buttontext,
            function cb() {
                document.location.href = Url;
            },
        );
    }
}
function ShowConfirmDialog(title, msg, confirmBtnText, cancelBtnText, successCallbackFunction, cancelCallbackFunction) {
    title = (title != "") ? title : 'Are you sure?';
    confirmBtnText = (confirmBtnText != "") ? confirmBtnText : 'Confirm';
    cancelBtnText = (cancelBtnText != "") ? cancelBtnText : 'Cancel';
    Notiflix.Confirm.show(
        title,
        msg,//"You won't be able to revert this!",
        confirmBtnText,
        cancelBtnText,
        function successCallback(){
            successCallbackFunction();
        },
        function cuccessCallback(){
            cancelCallbackFunction();
        }
    );
}

