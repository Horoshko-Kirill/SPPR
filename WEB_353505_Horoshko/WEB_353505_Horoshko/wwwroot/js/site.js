$(document).ready(function () {

    $(document).on("click", ".page-link", function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        $("#bookListContainer").load(url);
    });


    $(document).on("click", ".category-link", function (e) {
        e.preventDefault();
        var category = $(this).data("category");
        var url = category ? "/Catalog/" + encodeURIComponent(category) : "/Catalog";
        $("#bookListContainer").load(url);
        $(".dropdown-toggle").text($(this).text());
    });
});
