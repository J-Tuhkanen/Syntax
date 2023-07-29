class PostContentComponent {

    loadAmount = 20;
    excludedPosts = [];
    containerElement = null;
    requestUrl;

    Initialize(elementId) {

        this.containerElement = $(elementId);
        this.requestUrl = this.containerElement.data("post-request-url");
    }

    requestForPosts() {

        $.ajax({
            "type": "POST",
            "dataType": "json",
            "content-type": "application/json",
            "url": this.requestUrl,
            "data": JSON.stringify({

                ExcludedPosts: this.excludedPosts,
                Amount: this.loadAmount
            }),
            "headers": {

                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },

        })
            .done((data) => {

                $(this.containerElement).empty();
                this.buildPostElements(data);
            })
            .fail((data) => {

                console.log(data);
                alert("Failed to connect to API.");
            });
    }

    buildPostElements(data) {

        $.each(data, (index, value) => {

            this.excludedPosts.push(value.id);

            var postTitle = value.title;
            var postBody = value.body;
            var postId = value.id;
            var postAuthorId = value.userId;
            var postAuthor = value.userName;

            var postElement = $("<div class='main-page-post' data-view-post-url='post/" + postId + "'></div>");

            var postContentContainer = $("<div class='post-content'></div>");
            postContentContainer.append("<div class='post-sub-container header-container'><h3>" + postTitle + "</h3></div>")
            postContentContainer.append("<div class='post-sub-container'><p class='post-body'>" + postBody + "</p></div>")
            postContentContainer.append("<div class='post-sub-container author-container'><a class='post-author' href='profile/" + postAuthorId + "'>" + postAuthor + "</a></div>")
            postElement.append(postContentContainer);

            postElement.on('click', (e) => {

                var target = $(e.delegateTarget);

                window.location.href = target.data('view-post-url');
            });

            $(this.containerElement).append(postElement);
        });
    }
}

$(function () {

    var postContentComponent = new PostContentComponent();
    postContentComponent.Initialize("#index-page-posts-container");
    postContentComponent.requestForPosts();
});