class PostContentComponent {

    loadAmount = 5;
    childElementIdCollection = [];
    containerElement = null;
    requestUrl = null;

    Initialize(elementId) {

        this.containerElement = $(elementId);
        this.requestUrl = this.containerElement.data("post-request-url");
    }

    requestForPosts(clearExistingChildren = false) {

        $.ajax({
            async: true,
            "type": "POST",
            "dataType": "json",
            "content-type": "application/json",
            "url": this.requestUrl,
            "data": JSON.stringify({

                ExcludedPosts: this.childElementIdCollection,
                Amount: this.loadAmount
            }),
            "headers": {

                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },

        })
            .done((data) => {

                if (clearExistingChildren) {

                    this.clearElements();
                }
                this.buildPostElements(data);

                if (data.length > 0 && $(window).height() > this.containerElement.height()) {

                    this.requestForPosts();
                }
            })
            .fail(() => {


                alert("Server error.")
            });
    }

    clearElements() {

        this.containerElement.empty();
        this.childElementIdCollection = [];
    }

    buildPostElements(data) {

        $.each(data, (index, value) => {

            this.childElementIdCollection.push(value.Id);

            var postTitle = value.Title;
            var postBody = value.Body;
            var postId = value.Id;
            var postAuthorId = value.UserId;
            var postAuthor = value.UserName;

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

            this.containerElement.append(postElement);
        });
    }
}

var postContentComponent = new PostContentComponent();

$(function () {
    
    postContentComponent.Initialize("#index-page-posts-container"); 
    postContentComponent.requestForPosts(true);
});

$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() == $(document).height()) {

        var elementsBeforeRequest = postContentComponent.childElementIdCollection;

        postContentComponent.requestForPosts();

        var noMoreContentToLoad = postContentComponent.childElementIdCollection.length === elementsBeforeRequest.length;
        if (noMoreContentToLoad) {

            $(window).unbind("scroll");
        }
    }
});