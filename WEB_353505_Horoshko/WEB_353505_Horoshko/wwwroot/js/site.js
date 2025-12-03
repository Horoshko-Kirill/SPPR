$(document).ready(function () {


    $('#bookListContainer').on('click', '.page-link', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        $('#bookListContainer').load(url);
    });

    $('.category-link').click(function (e) {
        e.preventDefault();
        var category = $(this).data('category');
        var name = $(this).data('name'); 
        $('#currentCategoryName').text(name); 
        $('#bookListContainer').load('/Product?category=' + encodeURIComponent(category));
    });

});
