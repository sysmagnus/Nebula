**Edit a file, create a new file, and clone from Bitbucket in under 2 minutes**

When you're done, you can delete the content in this README and update the file with details for others getting started with your repository.

*We recommend that you open this README in another tab as you perform the tasks below. You can [watch our video](https://youtu.be/0ocf7u76WSo) for a full demo of all the steps in this tutorial. Open the video in a new tab to avoid leaving Bitbucket.*

---

## Edit a file

You’ll start by editing this README file to learn how to edit a file in Bitbucket.

1. Click **Source** on the left side.
2. Click the README.md link from the list of files.
3. Click the **Edit** button.
4. Delete the following text: *Delete this line to make a change to the README from Bitbucket.*
5. After making your change, click **Commit** and then **Commit** again in the dialog. The commit page will open and you’ll see the change you just made.
6. Go back to the **Source** page.

## Consulta Órdenes de reparación.
```
db.contacts.aggregate([
    {
        "$lookup": {
            "from": "order_repairs", "localField": "_id", "foreignField": "client_id", "as": "OrderRepair"
        }
    },
    {
        "$project": {
            "OrderRepair": {
                "$filter": {
                    "input": "$OrderRepair",
                    "as": "rep",
                    "cond": {
                        "$eq": ["$$rep.is_deleted", false]
                    }
                }
            }
        }
    },
    {
        "$unwind": "$OrderRepair"
    }
])
```