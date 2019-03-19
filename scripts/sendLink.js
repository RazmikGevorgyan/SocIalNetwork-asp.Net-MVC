$(document).ready(function () {
    let CurrentUserId = sessionStorage.getItem("id");
    $(".send-link").click(function () {
        $.ajax({
            type:"post",
            url: "/Auto_Notification_Sender/Index",
            data: { id: CurrentUserId },
            success: function () {
                alert("SuccessFuly send");
            },
        });
    })

});