(function () {
    $(document).ready(function () {

        var LogMeOut = function () {
            sessvars.$.clearMem();
            $.ajax
            (
                {
                    type: 'POST',
                    url: '/Account/LogOff',
                    success: function (msg) {
                        alert("Your session has expired due to an extended period of inactivity. You will need to reauthenticate to access the requested information.");
                        document.location = '/Account/Login';
                    },
                    error: function (result) {
                        alert(result);
                    }
                }
            )
        };

        setTimeout(LogMeOut, 28700000);

        // Create a jqxMenu
        $("#jqxMenu").jqxMenu({ width: '370', height: '30px' });
        $("#jqxMenu").css('visibility', 'visible');
        $("#jqxMenu").jqxMenu({ animationShowDuration: 300, animationHideDuration: 200, animationShowDelay: 200 });
        $("#jqxMenu").jqxMenu({ enableHover: true });
        $("#jqxMenu").jqxMenu({ autoOpen: true });
    });
} ()); 