/* Test whether the XUM1541/ZoomFloppy supports raw GCR track reads through the
   OpenCBM burst APIs. On this hardware both calls fail with LIBUSB_ERROR_NO_DEVICE. */
/* Build: gcc -O0 -g gcr_probe.c -lopencbm -o gcr_probe                         */
#include <opencbm.h>
#include <stdio.h>
#include <string.h>

int main(void)
{
    CBM_FILE f;
    unsigned char buf[8192];
    int rc;

    if (cbm_driver_open(&f, 0)) { printf("driver_open failed\n"); return 1; }
    memset(buf, 0, sizeof(buf));

    printf("trying cbm_parallel_burst_read_track...\n");
    rc = cbm_parallel_burst_read_track(f, buf, sizeof(buf));
    printf("parallel_burst rc=%d first16=", rc);
    for (int i = 0; i < 16; i++) printf("%02X ", buf[i]);
    printf("\n");

    memset(buf, 0, sizeof(buf));
    printf("trying cbm_srq_burst_read_track...\n");
    rc = cbm_srq_burst_read_track(f, buf, sizeof(buf));
    printf("srq_burst rc=%d first16=", rc);
    for (int i = 0; i < 16; i++) printf("%02X ", buf[i]);
    printf("\n");

    cbm_driver_close(f);
    return 0;
}
