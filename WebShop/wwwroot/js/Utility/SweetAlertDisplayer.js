"use strict";
class SweetAlertDisplayer {
    /**
     * Displays a SweetAlert modal with a confirmation and optional cancel button.
     *
     * @param {string} header - The title of the alert.
     * @param {string} info - The message displayed to the user.
     * @param {() => void} onConfirm - The callback function to execute when the "OK" button is clicked.
     *                                 It must be provided but can be an empty function if no action is needed.
     * @param {(() => void) | null} [onCancel=null] - An optional callback function for cancel logic.
     *                                               If provided, it enables the "Cancel" button.
     * @param {string} [confirmButtonText="OK"] - Custom text for the confirmation button. Defaults to `"OK"`.
     * @param {string} [cancelButtonText="Cancel"] - Custom text for the cancel button. Defaults to `"Cancel"`.
     *                                              Only applies if `onCancel` is provided.
     */
    FireSweetAlert(header, info, onConfirm, onCancel = null, confirmButtonText = "OK", cancelButtonText = "Cancel") {
        window.Swal.fire({
            title: header,
            text: info,
            icon: "info",
            showCancelButton: onCancel !== null,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: confirmButtonText,
            cancelButtonText: onCancel ? cancelButtonText : undefined // Ensures cancel button text is only set if it's used
        }).then((result) => {
            if (result.isConfirmed) {
                onConfirm();
            }
            else if (result.dismiss === window.Swal.DismissReason.cancel) {
                onCancel !== null && onCancel();
            }
        });
    }
}
//# sourceMappingURL=SweetAlertDisplayer.js.map