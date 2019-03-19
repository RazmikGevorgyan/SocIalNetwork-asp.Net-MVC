$(document).ready(function(){
    let CurrentUserId = sessionStorage.getItem("id");
    $(document).on('change', '.jscolor', function () {
        let colorVal ="#"+new String($(this).val());
        $(".btn-message").css({ "background-color": colorVal });
        $(".navbar").css({ "background-color": colorVal });
        $(".nav").css({ "background-color": colorVal });
    });
    $(document).on('click', '.change-color', function () {
        let id = new Number($(this).attr("data-id"));
        let color = "#" + new String($(".jscolor").val());
        $.ajax({
            type: "get",
            url: "/Chrono/ChangeColor/" + id,
            data:{color:color},
            success: function () {
            },
        });
    
    });
    $(window).resize(function () {
        var width = $(window).width();
        if (width < 600) {
            $(".normal").empty();
            $(".normal").remove();
            $(".navbar-brand").remove();
        } else {
            $(".for-small").empty();
            $(".for-small").remove();
            location.reload();
        }
    });
});
