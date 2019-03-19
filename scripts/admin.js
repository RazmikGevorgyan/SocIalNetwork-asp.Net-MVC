$(document).ready(function (){
    let CurrentUserId = sessionStorage.getItem("id");
    let id;
    let feedbackId
    $(document).on('click', '.deleted-feed', function () {
          id = $(this).attr("data-id");
    });
    $(document).on('click', '.give-feedback', function () {
        feedbackId= $(this).attr("data-id");
    });
    $(document).on('click', '#delete-and-send', function () {
        let val = document.querySelector('input[name="optradio"]:checked').value;
        if (val != undefined && val != null) {
            if (id != undefined && id != null) {
                $.ajax({
                    type: "post",
                    url: "/Chrono/DeleteFeed/" + id,
                    data: { value: val },
                    success: function () {
                        alert("done")
                        location.reload();
                    },
                });
            } else {
                $.ajax({
                    type: "post",
                    url: "/Admin/GetFeedback/" + feedbackId,
                    data: { value: val },
                    success: function () {
                        alert("done")
                        location.reload();
                    },
                });
            }
        }
    });
    let advId
    $(document).on('click', '.set-adv', function () {
        advId = new String($(this).attr("data-id"));
    });
    let sources = [];
    $("#file-input-adv").change(function (e) {
        var files = e.target.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                $(".picture-area").empty();
                $.ajax({
                    type: "POST",
                    url: "/Admin/UploadFile/",
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        let counter = 0
                        result.forEach(item=> {
                            if (item.photo != "") {
                                let source = item.photo
                                sources.push(source);
                                let src = item.photo;
                                src = src.replace('~', '');
                                src = "../.." + src;
                                $(".picture-area").append(`
                                    <div id="card-no-${counter}" class ="card col-md-4 h-100 w-100">
                                         <div class ="card-header btn btn-primary data-close-feed text-center"  data-id="${counter}" data-src="${source}" style="height:30px;">&times;</div>
                                         <img  class ="card-img-top content-src" data-src="${src}" style="height:150px;" src="${src}"/>
                                    </div>
                            `)
                                $(".share-area").empty();
                                $(".share-area").append(`
                             `)
                            }
                            counter++;
                        });
                        $(".share-area").append(
                        `<input class ="btn btn-primary" id="add-adv" type="submit" name="send" value="set" />`
                            )
                    },
                });
            } else {
                alert("Something went wrong! Please try again.");
            }
        }
    });
    $(document).on('click', '#add-adv', function () {
        let cost = new Number($(".input-cost").val());
        let url=new String($(".adv-url").val());
        if (advId != undefined && cost != undefined) {
            sources.forEach(item=> {
                $.ajax({
                    type: "Post",
                    url: "/Admin/AddAdv/"+advId,
                    data: {src:item ,cost:cost,url:url},
                    success: function () {
                        $(".picture-area").empty();
                        $("#file-input-adv").val("");
                        $(".adv-url").val("");
                        $(".picture-area").append(`<label class="text-success">successfuly done</label>`);
                        sources.length = 0;
                        location.reload();
                    },
                });
            });
        } else {
            $(".picture-area").empty();
            $(".picture-area").append(`<label class="text-danger">are you miss cost or upload image</label>`);
        }
    });
});