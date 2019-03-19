$(document).ready(function () {
    let CurrentUserId = sessionStorage.getItem("id");

    $(document).on("change", "#search", function () {
        let cont = new String($("#search").val());
        if (cont == null || cont == undefined || cont == "")
        {
            $(".serche-result").empty();
        }
    });
    $("#search").click(function () {
        let interval;
        clearInterval(interval);
        interval = setInterval(function () {
            let cont = new String($("#search").val());
            if (cont != null && cont != "") {
                $.ajax({
                    type: "post",
                    url: "/Profile/SearchResult/",
                    data: { cont: cont },
                    success: function (r) {
                        if (r.length != 0) {
                            r.forEach((item) => {
                                let name = item.name;
                                let surname = item.surname;
                                let id = item.id;
                                if ($("#user-" + id).length == 0) {
                                    $(".serche-result").append(`
                                    <div id="user-${id}" class ="dropdown-item">
                                         <a id="nohover" class ="btn btn-primary w-100" href="/Chrono/Index/${id}">${name} ${surname}</a><br>
                                    </div>
                                    `)
                                }
                            });
                            $(".serche-result").addClass("show");
                        }
                    }
                });
            } else
            {
                $(".serche-result").empty();
                $(".serche-result").removeClass("show");
            }
        }, 1000);
    });
});