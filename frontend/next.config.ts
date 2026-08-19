import type { NextConfig } from "next";
import { withSentryConfig } from "@sentry/nextjs";

const nextConfig: NextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "upload.wikimedia.org",
      },
    ],
  },
};

export default withSentryConfig(nextConfig, {
  org: "bunnys-e2",
  project: "success-lessons-frontend",
  // No SENTRY_AUTH_TOKEN is configured, so source map upload is skipped —
  // errors still report correctly, just with minified stack traces.
  silent: true,
  widenClientFileUpload: true,
});
