function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};

$(document).on('ready', function () {

    alert('hello');

    $(document).on('focus', '[id^=form-field]', function () {
        $(this).parent().removeClass("has-error");
        $(this).parent().find('.help-inline').hide();
    });

    $('.openEmail').on('click', function () {

        var title = "Suggestion";
        var isSuggestion = true;

        if ($(this).attr('id') == 'contactUs')
        {
            title = "Comments";
            isSuggestion = false;
        }
        
        var bootboxModal = bootbox.dialog({
            title: title,
            message: '<div class="row"><div class="col-md-12">'+
        '<div id="message-form-alert" class="alert alert-info">' +
        '</div>' +
        '<div class="form-group">'+
            '<label for="fullname">Name:</label>'+
            '<input type="text" name="fullname" class="form-control" placeholder="Enter here..." id="form-field-fullname">' +            
            '<span class="help-inline"></span>' +
        '</div>' +
        '<div class="form-group">' +
            '<label for="email">Email:</label>' +
            '<input type="email" name="email" class="form-control" placeholder="Enter here..." id="form-field-email">' +
            '<span class="help-inline"></span>' +
        '</div>' +
        '<div class="form-group">' +
            '<label for="email">' + title + ':</label>' +
            '<textarea name="comment" class="form-control" rows="3" placeholder="Enter here..." id="form-field-comment"></textarea>' +
            '<span class="help-inline"></span>' +
        '</div></div></div>',
            buttons: {
            success: {
            label: "Send Email",

            className: "btn btn-success pull-right emailSend",
        callback: function () {
            var fullname = $("#form-field-fullname");
            var email = $("#form-field-email");
            var comment = $("#form-field-comment");

            var msg1 = 'Please provide value for the required field';
            var msg2 = 'Please provide valid email address';
                    
            var errorFlag = false;

            if (fullname.val() == "") {
                errorFlag = true;                            
                $(fullname).parent().addClass('has-error');
                $(fullname).parent().find('.help-inline').text(msg1).show();
            }

            if (email.val() == "") {
                errorFlag = true;
                $(email).parent().addClass('has-error');
                $(email).parent().find('.help-inline').text(msg1).show();
            }
            else if (!isValidEmailAddress(email.val())) {
                $(email).parent().addClass('has-error');
                $(email).parent().find('.help-inline').text(msg2).show();
            }

            if (comment.val() == "") {
                errorFlag = true;
                $(comment).parent().addClass('has-error');
                $(comment).parent().find('.help-inline').text(msg1).show();
            }

            if (errorFlag) {
                $('.modal').animate({ scrollTop: 0 }, 'slow');
            }
            else {
                    $('.emailSend').text('Sending Email');

                    $.ajax({
                        type: "POST",
                        url: "/Home/SendEmail",
                        data: { "fullname": fullname.val(), "email": email.val(), "comment": comment.val(), 'isSuggestion': isSuggestion },
                        async: false,
                        success: function (result) {
                            if (result == "success") {
                                //$('#message-form-alert').html('Email Sent Successfully !!!').show();
                               
                                alert('Email Sent Successfully !!!');
                            }
                        },
                        error: function (errorThrown) {
                            $('.modal').animate({ scrollTop: 0 }, 'slow');
                        }
                    });
            }

            return !errorFlag;
        }
    },
        main: {
        label: "Close",
        className: "btn-primary emailClose",
        callback: function () {
        }
        }
}
});
});
});