$(document).ready(function () {
    let CurrentUserId = sessionStorage.getItem("id");
    $(document).on('click', '.share', function () {
        let id = new Number($(this).attr("data-id"));
        let CurrentUserId = sessionStorage.getItem("id");
        $.ajax({
            type: "get",
            data:{id:CurrentUserId},
            url: "/Chrono/SharePost/" + id,
            success: function () {
                location.reload();
            },
        });
    });
    $(document).on('click', '.share-photo', function () {
        let id = new Number($(this).attr("data-id"));
        $.ajax({
            type: "get",
            url: "/Chrono/SharePhoto/" + id,
            success: function () {
                location.reload();
            },

        });
    });
    $(document).on('click', '.share-url', function () {
        let id = new Number($(this).attr("data-id"));
        $.ajax({
            type: "get",
            url: "/Chrono/ShareUrl/" + id,
            success: function () {
                location.reload();
            },
        });
    });
    $(document).on('click', '.share-video', function () {
        let id = new Number($(this).attr("data-id"));
        $.ajax({
            type: "get",
            url: "/Chrono/ShareVideo/" + id,
            success: function () {
                location.reload();
            },
        });
    });
    $(document).on('click','.delete-from-timeline', function () {
        let id = new Number($(this).attr("data-id"));
        $.ajax({
            type: "get",
            url: "/Chrono/DeleteFeed/" + id,
            success: function () {
                alert("deleted");
                $(".container-"+id).remove();
            },

        });
    });
});