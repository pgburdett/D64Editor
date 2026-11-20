/* Direct libopencbm test: does EOI work when cbm_open() gets the filename? */
/* Build: gcc -O0 -g eoi_test.c -lopencbm -o eoi_test                         */
#include <opencbm.h>
#include <stdio.h>
#include <string.h>

int main(int argc, char **argv)
{
    CBM_FILE f;
    int rc = cbm_driver_open(&f, 0);
    printf("driver_open=%d\n", rc);
    if (rc) return 1;

    const char *name = argc > 1 ? argv[1] : "$";
    printf("cbm_open(dev=8, sec=0, name=\"%s\", len=%zu)\n", name, strlen(name));
    printf("open=%d\n", cbm_open(f, 8, 0, name, strlen(name)));
    printf("talk=%d\n", cbm_talk(f, 8, 0));

    for (int i = 0; i < 600; i++)
    {
        unsigned char c = 0;
        int n = cbm_raw_read(f, &c, 1);
        int eoi = cbm_get_eoi(f);
        printf("%04d n=%d eoi=%d byte=%02X '%c'\n", i, n, eoi, c,
               (c >= 32 && c < 127) ? (char)c : '.');
        if (eoi || n <= 0) break;
    }

    cbm_untalk(f);
    cbm_close(f, 8, 0);
    cbm_driver_close(f);
    return 0;
}
