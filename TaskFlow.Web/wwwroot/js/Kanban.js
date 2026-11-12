// ======================================
// TASKFLOW - KANBAN BOARD (DRAG & DROP + SIGNALR + ADD TASK)
// ======================================

document.addEventListener("DOMContentLoaded", function () {

    const board = document.getElementById("kanbanBoard");

    if (!board) return;

    let draggedCard = null;

    // Get all columns
    function getColumns() {
        return board.querySelectorAll(".kanban-column");
    }

    // Update task count in every column
    function updateColumnCounts() {
        getColumns().forEach(function (column) {
            const tasksContainer = column.querySelector(".kanban-tasks");
            const countElement = column.querySelector(".kanban-column-count");

            if (!tasksContainer || !countElement) return;

            const cards = tasksContainer.querySelectorAll(".kanban-card");
            countElement.textContent = cards.length;
        });
    }

    // Make updateColumnCounts globally accessible
    window.updateColumnCounts = updateColumnCounts;

    // DRAG START
    board.addEventListener("dragstart", function (event) {
        const card = event.target.closest(".kanban-card");

        if (!card) return;

        draggedCard = card;
        card.classList.add("dragging");

        event.dataTransfer.effectAllowed = "move";
        event.dataTransfer.setData(
            "text/plain",
            card.dataset.taskId || ""
        );
    });

    // DRAG END
    board.addEventListener("dragend", function () {
        if (draggedCard) {
            draggedCard.classList.remove("dragging");
        }

        getColumns().forEach(function (column) {
            column.classList.remove("drag-over");
        });

        draggedCard = null;
    });

    // DRAG OVER
    board.addEventListener("dragover", function (event) {
        const column = event.target.closest(".kanban-column");

        if (!column || !draggedCard) return;

        event.preventDefault();
        event.dataTransfer.dropEffect = "move";

        getColumns().forEach(function (item) {
            item.classList.remove("drag-over");
        });

        column.classList.add("drag-over");
    });

    // DRAG LEAVE
    board.addEventListener("dragleave", function (event) {
        const column = event.target.closest(".kanban-column");

        if (!column) return;

        if (column.contains(event.relatedTarget)) return;

        column.classList.remove("drag-over");
    });

    // DROP
    board.addEventListener("drop", function (event) {
        const column = event.target.closest(".kanban-column");

        if (!column || !draggedCard) return;

        event.preventDefault();

        const tasksContainer = column.querySelector(".kanban-tasks");

        if (!tasksContainer) return;

        // Move the task to the destination column
        tasksContainer.appendChild(draggedCard);

        // Update the task's current status on the page
        const taskId = draggedCard.dataset.taskId;
        const newStatus = column.dataset.column;
        draggedCard.dataset.status = newStatus;

        // Remove visual drag styles
        column.classList.remove("drag-over");
        draggedCard.classList.remove("dragging");

        // Update all column counters
        updateColumnCounts();

        // SIGNALR: إرسال التحديث للسيرفر لحظياً
        if (window.kanbanConnection && window.kanbanConnection.state === signalR.HubConnectionState.Connected) {
            window.kanbanConnection.invoke("UpdateTaskStatus", taskId, newStatus)
                .catch(function (err) {
                    console.error("Error updating task status via SignalR: ", err.toString());
                });
        }

        draggedCard = null;
    });

    // TASK SEARCH
    const searchInput = document.getElementById("kanbanSearch");

    if (searchInput) {
        searchInput.addEventListener("input", function () {
            const searchValue = this.value.trim().toLowerCase();
            const cards = board.querySelectorAll(".kanban-card");

            cards.forEach(function (card) {
                const title = card.querySelector(
                    ".kanban-card-title-text"
                )?.textContent.toLowerCase() || "";

                const description = card.querySelector(
                    ".kanban-card-desc-text"
                )?.textContent.toLowerCase() || "";

                const matches =
                    title.includes(searchValue) ||
                    description.includes(searchValue);

                card.style.display = matches ? "" : "none";
            });
        });
    }

    // Set initial counts
    updateColumnCounts();

    // ==========================================
    // ADD NEW TASK LOGIC (MODAL SUBMIT)
    // ==========================================
    const saveTaskBtn = document.getElementById("saveTaskBtn");

    if (saveTaskBtn) {
        saveTaskBtn.addEventListener("click", function () {
            const titleInput = document.getElementById("taskTitle");
            const descInput = document.getElementById("taskDesc");
            const statusSelect = document.getElementById("taskStatus");
            const categorySelect = document.getElementById("taskCategory");
            const prioritySelect = document.getElementById("taskPriority");

            if (!titleInput.value.trim()) {
                alert("Please enter a task title!");
                titleInput.focus();
                return;
            }

            const newTaskId = "task-" + Date.now();
            const title = titleInput.value.trim();
            const description = descInput.value.trim();
            const targetStatus = statusSelect.value;
            const category = categorySelect.value;
            const priority = prioritySelect.value;

            // تحديد فئة التصنيف للـ CSS class
            let categoryClass = "category-development";
            if (category === "Design") categoryClass = "category-design";
            else if (category === "Research") categoryClass = "category-research";
            else if (category === "Marketing") categoryClass = "category-marketing";

            // تصميم كارت المهمة الجديد HTML
            const cardHTML = `
                <article class="kanban-card" draggable="true" data-task-id="${newTaskId}" data-status="${targetStatus}">
                    <div class="kanban-card-top">
                        <span class="kanban-card-category ${categoryClass}">
                            ${category}
                        </span>
                        <button class="kanban-card-menu" type="button">
                            <i class="fa-solid fa-ellipsis"></i>
                        </button>
                    </div>

                    <h4 class="kanban-card-title kanban-card-title-text">
                        ${title}
                    </h4>

                    <p class="kanban-card-description kanban-card-desc-text">
                        ${description || "No description provided."}
                    </p>

                    <div class="kanban-card-footer">
                        <span class="kanban-priority priority-${priority.toLowerCase()}">
                            <i class="fa-solid fa-flag"></i>
                            ${priority}
                        </span>

                        <div class="kanban-card-meta">
                            <span class="kanban-meta-item">
                                <i class="fa-regular fa-clock"></i>
                                Just now
                            </span>
                        </div>
                    </div>
                </article>
            `;

            // إدراج الكارت الجديد في العمود المحدد
            const targetColumn = document.getElementById(targetStatus) || document.querySelector(`.kanban-column[data-column="${targetStatus}"]`);
            if (targetColumn) {
                const tasksContainer = targetColumn.querySelector(".kanban-tasks");
                if (tasksContainer) {
                    tasksContainer.insertAdjacentHTML("beforeend", cardHTML);
                    updateColumnCounts();

                    // إغلاق الـ Modal وتفريغ الحقول
                    const modalElement = document.getElementById("addTaskModal");
                    const modalInstance = bootstrap.Modal.getInstance(modalElement);
                    if (modalInstance) {
                        modalInstance.hide();
                    }

                    document.getElementById("addTaskForm").reset();
                    console.log(`New task created in [${targetStatus}] successfully: ${newTaskId}`);
                }
            }
        });
    }

    // ==========================================
    // SIGNALR: إعداد اتصال الـ Hub واستقبال التحديثات
    // ==========================================
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/kanbanHub")
        .withAutomaticReconnect()
        .build();

    window.kanbanConnection = connection;

    // استقبال التحديثات من باقي الأعضاء لحظياً
    connection.on("ReceiveTaskMove", function (taskId, newStatus) {
        const taskCard = document.querySelector(`[data-task-id="${taskId}"]`);
        const targetColumn = document.querySelector(`.kanban-column[data-column="${newStatus}"]`);

        if (targetColumn && taskCard) {
            const tasksContainer = targetColumn.querySelector(".kanban-tasks");
            if (tasksContainer) {
                tasksContainer.appendChild(taskCard);
                taskCard.dataset.status = newStatus;
                updateColumnCounts();
                console.log(`Real-time sync: Task ${taskId} moved to ${newStatus}`);
            }
        }
    });

    // بدء الاتصال بالسيرفر
    connection.start().then(function () {
        console.log("SignalR Connected Successfully!");
    }).catch(function (err) {
        console.error("SignalR Connection Failed: ", err.toString());
    });

});