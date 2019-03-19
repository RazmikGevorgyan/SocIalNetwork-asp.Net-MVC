$(document).ready(function () {
    let CurrentUserId = sessionStorage.getItem("id");
    $(document).on('click', '.from-exciting', function () {
        let id = new Number($(this).attr("data-id"));
        let type=new String($(this).attr("data-type"));
        $(".photo-modal").empty();
        $.ajax({
            type: "post",
            url: "/Chrono/GetPhoto/" + id,
            success: function (r) {
                if (r.length > 0) {
                    r.forEach((item) => {
                        let photo = item.photo;
                        let id1 = item.id1;
                        photo = photo.replace('~','');
                        let src = "../.." + photo;
                        $(".photo-modal").append(`
                          <input type="radio" name="emotion" data-id="${id1}" id="check-this-${id1}" class ="input-hidden" />
                                 <label for="check-this-${id1}">
                                        <img src="${src}"/>
                                 </label>
                        `)
                    });
                }
            }
        })
        if (type == "cover") {
            $(".modal-smood").empty();
            $(".modal-smood").append(`
                   <button class ="btn btn-primary confirm-change-cover">confirm</button>
                   <button type="button" class ="btn btn-danger" data-dismiss="modal">Close</button>
                `)
        } else {
            $(".modal-smood").empty();
            $(".modal-smood").append(`
                   <button class ="btn btn-primary confirm-change-profile">confirm</button>
                    <button type="button" class ="btn btn-danger" data-dismiss="modal">Close</button>
                `)
        }
    });

    $(document).on('click', '.confirm-change-cover', function () {
        var feedId = $("input[name='emotion']:checked").attr("data-id");
        $.ajax({
            type: "post",
            url: "/Chrono/ChangeCover/" + feedId,
            success: function () {
                location.reload();
            },
        })
    });
    $(document).on('click', '.confirm-change-profile', function () {
        var feedId = $("input[name='emotion']:checked").attr("data-id");
        $.ajax({
            type: "post",
            url: "/Chrono/ChangeProfile/" + feedId,
            success: function () {
                location.reload();
            },
        })
    });
});